
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../services/user.service';
import { TaskService } from '../../services/task.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent implements OnInit {

  totalUsers = 0;
  totalTasks = 0;
  errorMessage = '';

  constructor(
    private userService: UserService,
    private taskService: TaskService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadTasks();
  }

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        // Supports an API returning a user array
        this.totalUsers = Array.isArray(res)
          ? res.length
          : Array.isArray(res.users)
            ? res.users.length
            : 0;
      },
      error: () => {
        this.errorMessage = 'Unable to load users';
      }
    });
  }

  loadTasks(): void {
    this.taskService.getAllTasks(1, 10).subscribe({
      next: (res) => {
        // Total count from paginated API response
        this.totalTasks = res.totalCount ?? 0;
      },
      error: () => {
        this.errorMessage = 'Unable to load tasks';
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

