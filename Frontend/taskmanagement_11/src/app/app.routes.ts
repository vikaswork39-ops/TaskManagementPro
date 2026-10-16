
import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { UserDashboardComponent } from './components/user-dashboard/user-dashboard.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { CreateTaskComponent } from './components/create-task/create-task.component';
import { EditTaskComponent } from './components/edit-task/edit-task.component';

import { authGuard } from './guards/auth.guard';

export const routes: Routes = [

  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  {
    path: 'admin-dashboard',
    component: AdminDashboardComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] }
  },

  {
    path: 'user-dashboard',
    component: UserDashboardComponent,
    canActivate: [authGuard],
    data: { roles: ['User'] }
  },

  {
    path: 'tasks',
    component: TaskListComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] }
  },

  {
    path: 'create-task',
    component: CreateTaskComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] }
  },

  {
    path: 'edit-task/:id',
    component: EditTaskComponent,
    canActivate: [authGuard],
    data: { roles: ['Admin'] }
  },

  { path: '**', redirectTo: 'login' }
];

