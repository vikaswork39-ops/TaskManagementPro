
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-edit-task',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './edit-task.component.html',
  styleUrl: './edit-task.component.css'
})
export class EditTaskComponent implements OnInit {

  taskForm: FormGroup;
  users: any[] = [];
  taskId = 0;
  priorityOptions: any[] = [];
statusOptions: any[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    private userService: UserService
  ) {
    this.taskForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      priority: [null, Validators.required],
      status: [null, Validators.required],
      assignedUserId: [null, Validators.required]
    });
  }

 ngOnInit(): void {
  this.taskId = Number(this.route.snapshot.paramMap.get('id'));

  if (!this.taskId) {
    this.errorMessage = 'Invalid task ID';
    return;
  }

  this.loadPriorityOptions();
  this.loadStatusOptions();
  this.loadUsers();
  this.loadTask();
}

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (res) => {
        this.users = Array.isArray(res) ? res : res.data ?? [];
      },
      error: () => {
        this.errorMessage = 'Unable to load users';
      }
    });
  }

  loadTask(): void {
    this.taskService.getTaskById(this.taskId).subscribe({
      next: (res) => {
        this.taskForm.patchValue({
          title: res.title,
          description: res.description,
          priority: res.priority,
          status: res.status,
          assignedUserId: res.assignedUserId
        });
      },
      error: () => {
        this.errorMessage = 'Unable to load task details';
      }
    });
  }

  onUpdate(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.updateTask(
      this.taskId,
      this.taskForm.value
    ).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage =
          err.error?.message || 'Unable to update task';
      }
    });
  }
  
loadPriorityOptions(): void {
  this.taskService.getPriorityOptions().subscribe({
    next: (res) => {
      this.priorityOptions = res;
    },
    error: (err) => {
      console.error(err);
      this.errorMessage = 'Unable to load priorities';
    }
  });
}

loadStatusOptions(): void {
  this.taskService.getStatusOptions().subscribe({
    next: (res) => {
      this.statusOptions = res;
    },
    error: (err) => {
      console.error(err);
      this.errorMessage = 'Unable to load statuses';
    }
  });
}

}
