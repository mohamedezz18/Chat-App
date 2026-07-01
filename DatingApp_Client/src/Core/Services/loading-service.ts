import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  public count = signal(0);

  start() {
    this.count.update((n) => n + 1);
  }
  stop() {
    this.count.update((n) => Math.max(0, n - 1));
  }
}
