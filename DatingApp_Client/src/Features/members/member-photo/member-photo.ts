import { AsyncPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../../../Core/Services/member-service';
import { Observable } from 'rxjs';
import { Member, Photo } from '../../../Models/Member';
import { ImageUpload } from '../../../Shared/image-upload/image-upload';
import { AccountService } from '../../../Core/Services/account-service';
import { User } from '../../../Models/User';
import { StarButton } from '../../../Shared/star-button/star-button';
import { DeleteButton } from '../../../Shared/delete-button/delete-button';

@Component({
  selector: 'app-member-photo',
  imports: [ImageUpload, StarButton, DeleteButton],
  templateUrl: './member-photo.html',
  styleUrl: './member-photo.css',
})
export class MemberPhoto implements OnInit {
  protected _ActivateRoute = inject(ActivatedRoute);
  protected _MemberService = inject(MemberService);
  protected _AccountService = inject(AccountService);
  protected Photos = signal<Photo[]>([]);
  protected loading = signal(false);

  ngOnInit(): void {
    const memberId = this._ActivateRoute.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this._MemberService.getMemberPhotos(memberId).subscribe({
        next: (photos) => this.Photos.set(photos),
      });
    }
  }

  onUploadImage(file: File) {
    this.loading.set(true);
    this._MemberService.uploadPhoto(file).subscribe({
      next: (photo) => {
        this._MemberService.EditMode.set(false);
        if (this.Photos().length == 0) {
          this.setMainPhoto(photo);
        }
        this.loading.set(false);
        this.Photos.update((photos) => [...photos, photo]);
        if (!this._MemberService.member()?.imageUrl) {
          this.mainPhoto(photo);
        }
      },
      error: (error) => {
        console.log('Error uploading image: ', error);
        this.loading.set(false);
      },
    });
  }

  setMainPhoto(photo: Photo) {
    this._MemberService.setMainPhoto(photo).subscribe({
      next: () => {
        this.mainPhoto(photo);
      },
    });
  }

  deletePhoto(photoId: number) {
    this._MemberService.DeletePhoto(photoId).subscribe({
      next: () => {
        this.Photos.update((photos) => photos.filter((x) => x.id !== photoId));
      },
    });
  }

  private mainPhoto(photo: Photo) {
    const currentUser = this._AccountService.currentUser();
    if (currentUser) currentUser.imageUrl = photo.url;
    this._AccountService.setCurrentUser(currentUser as User);
    this._MemberService.member.update(
      (member) =>
        ({
          ...member,
          imageUrl: photo.url,
        }) as Member,
    );
  }
}
