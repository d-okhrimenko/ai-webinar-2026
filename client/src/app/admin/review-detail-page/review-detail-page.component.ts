import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
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
  readonly isAnalyzing = signal(false);
  readonly isGeneratingResponse = signal(false);
  readonly actionError = signal('');

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.api.get(id).subscribe({
      next: (review) => this.review.set(review),
      error: () => this.error.set('Відгук не знайдено.'),
    });
  }

  analyze(): void {
    const review = this.review();
    if (!review || this.isAnalyzing()) return;

    this.isAnalyzing.set(true);
    this.actionError.set('');
    this.api
      .analyze(review.id)
      .pipe(finalize(() => this.isAnalyzing.set(false)))
      .subscribe({
        next: (updatedReview) => this.review.set(updatedReview),
        error: () => this.actionError.set('Не вдалося виконати аналіз відгуку.'),
      });
  }

  generateDraftResponse(): void {
    const review = this.review();
    if (!review || this.isGeneratingResponse()) return;

    this.isGeneratingResponse.set(true);
    this.actionError.set('');
    this.api
      .generateDraftResponse(review.id)
      .pipe(finalize(() => this.isGeneratingResponse.set(false)))
      .subscribe({
        next: (updatedReview) => this.review.set(updatedReview),
        error: () => this.actionError.set('Не вдалося згенерувати чернетку відповіді.'),
      });
  }
}
