import { Component, OnInit, inject, signal } from '@angular/core';
import { LikesService } from '../../Core/Services/likes-service';
import { Member } from '../../Models/Member';
import { MemberCard } from '../members/member-card/member-card';
import { PaginatedResult } from '../../Models/pagination';
import { Paginator } from '../../Shared/paginator/paginator';

@Component({
  selector: 'app-lists',
  imports: [MemberCard, Paginator],
  templateUrl: './lists.html',
  styleUrl: './lists.css',
})
export class Lists implements OnInit {
  private likesService = inject(LikesService);
  protected paginatedResult = signal<PaginatedResult<Member> | null>(null);
  protected predicate = signal<string>('liked');
  protected pageNumber = 1;
  protected pageSize = 5;

  tabs = [
    { label: 'Liked', value: 'liked' },
    { label: 'Liked me', value: 'likedBy' },
    { label: 'Mutual', value: 'mutual' },
  ];

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string) {
    if (this.predicate() !== predicate) {
      this.predicate.set(predicate);
      this.pageNumber = 1;
      this.loadLikes();
    }
  }

  loadLikes() {
    this.likesService.getLikes(this.predicate(), this.pageNumber, this.pageSize).subscribe({
      next: (response) => this.paginatedResult.set(response),
    });
  }

  onPageChange(event: { pageNumber: number; pageSize: number }) {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageNumber;
    this.loadLikes();
  }
}
