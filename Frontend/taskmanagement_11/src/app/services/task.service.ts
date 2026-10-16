
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private apiUrl = 'https://localhost:7158/api/task';

  constructor(private http: HttpClient) {}

  
getAllTasks(
  pageNumber: number,
  pageSize: number,
  search: string = '',
   status: number | null = null,
  priority: number | null = null
): Observable<any> {

  let params: any = {
    pageNumber,
    pageSize
  };

  if (search.trim()) {
    params.search = search.trim();
  }

  if (status) {
    params.status = status;
  }

  if (priority) {
    params.priority = priority;
  }

  return this.http.get<any>(
    `${this.apiUrl}/get-all`,
    { params }
  );
}

createTask(data: any): Observable<any> {
  return this.http.post<any>(
    `${this.apiUrl}/create`,
    data
  );
}

getTaskById(id: number): Observable<any> {
  return this.http.get<any>(
    `${this.apiUrl}/get-by-id/${id}`
  );
}

updateTask(id: number, data: any): Observable<any> {
  return this.http.put<any>(
    `${this.apiUrl}/update/${id}`,
    data
  );
}

getMyTasks(): Observable<any> {
  return this.http.get<any>(
    `${this.apiUrl}/my-tasks`
  );
}

updateTaskStatus(id: number, status: number): Observable<any> {
  return this.http.put<any>(
    `${this.apiUrl}/update-status/${id}`,
    { status }
  );
}

getStatusOptions(): Observable<any[]> {
  return this.http.get<any[]>(
    `${this.apiUrl}/status-options`
  );
}

getPriorityOptions(): Observable<any[]> {
  return this.http.get<any[]>(
    `${this.apiUrl}/priority-options`
  );
}


deleteTask(id: number): Observable<any> {
  return this.http.delete<any>(
    `${this.apiUrl}/delete/${id}`
  );
}

}
