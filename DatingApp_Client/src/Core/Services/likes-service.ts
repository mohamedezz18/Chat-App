import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../Environments/environment.development';
import { Member } from '../../Models/Member';
import { tap } from 'rxjs';
import { PaginatedResult } from '../../Models/pagination';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  private http = inject(HttpClient);
  private baseUrl = environment.BaseUrl;
  public likeIds = signal<string[]>([]);

  toggleLike(targeMemberID: string) {
    return this.http.post(this.baseUrl + 'likes/' + targeMemberID, {}).subscribe({
      next: () => {
        if (this.likeIds().includes(targeMemberID)) {
          this.likeIds.update((ids) => ids.filter((x) => x !== targeMemberID));
        } else {
          this.likeIds.update((ids) => [...ids, targeMemberID]);
        }
      },
    });
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    params = params.append('predicate', predicate);

    return this.http.get<PaginatedResult<Member>>(this.baseUrl + 'likes', { params });
  }

  getLIkeIds() {
    return this.http.get<string[]>(this.baseUrl + 'likes/lists').subscribe({
      next: (response) => {
        this.likeIds.set(response);
      },
    });
  }

  clearLikeIds() {
    this.likeIds.set([]);
  }
}
