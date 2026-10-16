
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { TaskService } from '../../services/task.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.css'
})
export class UserDashboardComponent implements OnInit {

  tasks: any[] = [];

  priorityOptions: any[] = [];
  statusOptions: any[] = [];

  isLoading = false;
  updatingTaskId: number | null = null;

  errorMessage = '';
  successMessage = '';

  constructor(
    private taskService: TaskService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPriorityOptions();
    this.loadStatusOptions();
    this.loadMyTasks();
  }

  // Load assigned tasks
  loadMyTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.getMyTasks().subscribe({
      next: (res) => {
        this.tasks = Array.isArray(res)
          ? res
          : res.data ?? [];

        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Unable to load assigned tasks';
        console.error(err);
      }
    });
  }

  // Load priority options from API
  loadPriorityOptions(): void {
    this.taskService.getPriorityOptions().subscribe({
      next: (res) => {
        this.priorityOptions = res;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Unable to load priority options';
      }
    });
  }

  // Load status options from API
  loadStatusOptions(): void {
    this.taskService.getStatusOptions().subscribe({
      next: (res) => {
        this.statusOptions = res;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Unable to load status options';
      }
    });
  }

  // Get dynamic priority name
  getPriorityName(id: number): string {
    return this.priorityOptions.find(
      item => item.id === id
    )?.name ?? 'Unknown';
  }

  // Get dynamic status name
  getStatusName(id: number): string {
    return this.statusOptions.find(
      item => item.id === id
    )?.name ?? 'Unknown';
  }

  // Update task status
  updateStatus(task: any, newStatus: number): void {

    if (task.status === newStatus) {
      return;
    }

    this.updatingTaskId = task.id;
    this.errorMessage = '';
    this.successMessage = '';

    this.taskService.updateTaskStatus(
      task.id,
      newStatus
    ).subscribe({
      next: () => {
        this.updatingTaskId = null;
        this.successMessage =
          'Task status updated successfully';

        this.loadMyTasks();
      },
      error: (err) => {
        this.updatingTaskId = null;
        this.errorMessage =
          err.error?.message ||
          'Unable to update task status';

        console.error(err);
      }
    });
  }

  // Logout
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
