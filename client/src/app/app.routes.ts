import { Routes } from '@angular/router';
import { adminGuard } from './core/auth';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'feedback' },
  {
    path: 'feedback',
    loadComponent: () =>
      import('./feedback/feedback-page/feedback-page.component').then((m) => m.FeedbackPage),
  },
  {
    path: 'login',
    loadComponent: () => import('./login/login-page/login-page.component').then((m) => m.LoginPage),
  },
  {
    path: 'admin/reviews',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./admin/reviews-page/reviews-page.component').then((m) => m.ReviewsPage),
  },
  {
    path: 'admin/reviews/:id',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./admin/review-detail-page/review-detail-page.component').then(
        (m) => m.ReviewDetailPage,
      ),
  },
  { path: '**', redirectTo: 'feedback' },
];
