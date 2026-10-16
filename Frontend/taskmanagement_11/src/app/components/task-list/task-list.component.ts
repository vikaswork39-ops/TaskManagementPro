
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {

  tasks: any[] = [];

  statusOptions: any[] = [];
  priorityOptions: any[] = [];

  search = '';
  status: number | null = null;
  priority: number | null = null;

  pageNumber = 1;
  pageSize = 10;
  totalCount = 0;

  isLoading = false;
  errorMessage = '';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadStatusOptions();
    this.loadPriorityOptions();
    this.loadTasks();
  }

  // Load tasks from API
  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.getAllTasks(
      this.pageNumber,
      this.pageSize,
      this.search,
      this.status,
      this.priority
    ).subscribe({
      next: (res) => {
        this.tasks = res.data ?? [];
        this.totalCount = res.totalRecords ?? 0;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Unable to load tasks';
        console.error(err);
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

  // Get status name from API options
  getStatusName(id: number): string {
    const item = this.statusOptions.find(
      x => x.id === id
    );

    return item?.name ?? 'Unknown';
  }

  // Get priority name from API options
  getPriorityName(id: number): string {
    const item = this.priorityOptions.find(
      x => x.id === id
    );

    return item?.name ?? 'Unknown';
  }

  // Priority badge color
  getPriorityClass(id: number): string {
    const name = this.getPriorityName(id);

    switch (name) {
      case 'High':
        return 'bg-danger-subtle text-danger';
      case 'Medium':
        return 'bg-warning-subtle text-warning-emphasis';
      case 'Low':
        return 'bg-success-subtle text-success';
      default:
        return 'bg-secondary-subtle text-secondary';
    }
  }

  // Status badge color
  getStatusClass(id: number): string {
    const name = this.getStatusName(id);

    switch (name) {
      case 'Pending':
        return 'bg-warning-subtle text-warning-emphasis';
      case 'InProgress':
        return 'bg-info-subtle text-info-emphasis';
      case 'Completed':
        return 'bg-success-subtle text-success';
      default:
        return 'bg-secondary-subtle text-secondary';
    }
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadTasks();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.loadTasks();
    }
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadTasks();
    }
  }

  deleteTask(id: number): void {
    if (!confirm('Are you sure you want to delete this task?')) {
      return;
    }

    this.taskService.deleteTask(id).subscribe({
      next: () => {
        if (this.tasks.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }

        this.loadTasks();
      },
      error: (err) => {
        this.errorMessage = 'Unable to delete task';
        console.error(err);
      }
    });
  }
}
