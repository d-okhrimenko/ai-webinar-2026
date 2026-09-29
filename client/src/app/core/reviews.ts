import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { CreateReviewRequest } from './models/review/create-review-request.model';
import { Review } from './models/review/review.model';

@Injectable({ providedIn: 'root' })
export class ReviewsApi {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/api`;

  create(input: CreateReviewRequest) {
    return this.http.post<Review>(`${this.apiUrl}/reviews`, input);
  }

  list(filters: Record<string, string> = {}) {
    return this.http.get<Review[]>(`${this.apiUrl}/admin/reviews`, { params: filters });
  }

  get(id: string) {
    return this.http.get<Review>(`${this.apiUrl}/admin/reviews/${id}`);
  }
}
