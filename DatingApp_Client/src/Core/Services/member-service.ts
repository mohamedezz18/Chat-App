import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../Environments/environment.development';
import { AccountService } from './account-service';
import { Member, MemberParams, MemberUpdate, Photo } from '../../Models/Member';
import { tap } from 'rxjs';
import { PaginatedResult } from '../../Models/pagination';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  //private _accountService = inject(AccountService);
  private baseUrl = environment.BaseUrl;
  public EditMode = signal(false);
  public member = signal<Member | null>(null);

  getMembers(memberParams: MemberParams) {
    let params = new HttpParams();

    params = params.append('pageNumber', memberParams.pageNumber);
    params = params.append('pageSize', memberParams.pageSize);
    params = params.append('minAge', memberParams.minAge);
    params = params.append('maxAge', memberParams.maxAge);
    params = params.append('orderBy', memberParams.orderBy);
    if (memberParams.gender) params = params.append('gender', memberParams.gender);
    return this.http.get<PaginatedResult<Member>>(this.baseUrl + `members`, { params }).pipe(
      tap(() => {
        localStorage.setItem('filters', JSON.stringify(memberParams));
      }),
    );
  }

  getMember(id: string) {
    return this.http.get<Member>(this.baseUrl + `members/${id}`).pipe(
      tap((_member) => {
        this.member.set(_member);
      }),
    );
  }

  getMemberPhotos(id: string) {
    return this.http.get<Photo[]>(this.baseUrl + `members/${id}` + '/photos');
  }

  UpdateMember(memberUpdate: MemberUpdate) {
    return this.http.put(this.baseUrl + 'members', memberUpdate);
  }

  uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'members/add-photo', formData);
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'members/set-main-photo/' + photo.id, {});
  }

  DeletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'members/delete-photo/' + photoId);
  }
}
