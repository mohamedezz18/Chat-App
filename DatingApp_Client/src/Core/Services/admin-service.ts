import { Injectable, inject } from '@angular/core';
import { environment } from '../../Environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { User } from '../../Models/User';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private baseUrl = environment.BaseUrl;
  private http = inject(HttpClient);

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(userId: string, roles: string[]) {
    return this.http.post<string[]>(
      this.baseUrl + 'admin/edit-roles/' + userId + '?roles=' + roles,
      {},
    );
  }
}
