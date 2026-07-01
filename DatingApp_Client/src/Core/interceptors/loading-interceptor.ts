import { HttpInterceptorFn } from '@angular/common/http';
import { LoadingService } from '../Services/loading-service';
import { inject } from '@angular/core';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const _loadingService = inject(LoadingService);

  _loadingService.start();

  return next(req).pipe(
    delay(200),
    finalize(() => {
      _loadingService.stop();
    }),
  );
};
