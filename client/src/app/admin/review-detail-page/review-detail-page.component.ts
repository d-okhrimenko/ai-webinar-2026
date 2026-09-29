import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Review } from '../../core/models/review/review.model';
import { ReviewsApi } from '../../core/reviews';

@Component({
  selector: 'app-review-detail-page',
  imports: [RouterLink],
  templateUrl: './review-detail-page.component.html',
  styleUrl: './review-detail-page.component.css',
})
export class ReviewDetailPage {
  private readonly api = inject(ReviewsApi);
  private readonly route = inject(ActivatedRoute);

  readonly review = signal<Review | null>(null);
  readonly error = signal('');

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.api.get(id).subscribe({
      next: (review) => this.review.set(review),
      error: () => this.error.set('Відгук не знайдено.'),
    });
  }
}
