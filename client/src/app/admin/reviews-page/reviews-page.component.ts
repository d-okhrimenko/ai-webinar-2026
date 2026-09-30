import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  analysisStatusLabels,
  categoryLabels,
  priorityLabels,
  sentimentLabels,
} from '../../core/localization/review-labels';
import { AnalysisStatus } from '../../core/models/review/analysis-status.model';
import { Priority } from '../../core/models/review/priority.model';
import { Review } from '../../core/models/review/review.model';
import { ReviewCategory } from '../../core/models/review/review-category.model';
import { Sentiment } from '../../core/models/review/sentiment.model';
import { ReviewsApi } from '../../core/reviews';

@Component({
  selector: 'app-reviews-page',
  imports: [RouterLink, DatePipe],
  templateUrl: './reviews-page.component.html',
  styleUrl: './reviews-page.component.css',
})
export class ReviewsPage {
  private readonly api = inject(ReviewsApi);
  private readonly filters: Record<string, string> = {};

  readonly reviews = signal<Review[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');

  constructor() {
    this.load();
  }

  filter(name: string, event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value) this.filters[name] = value;
    else delete this.filters[name];
    this.load();
  }

  sortByPriority(): void {
    this.filters['sortBy'] = 'priority';
    this.load();
  }

  reviewPreview(text: string): string {
    const maxLength = 50;
    return text.length > maxLength ? `${text.slice(0, maxLength)}…` : text;
  }

  analysisStatusLabel(status: AnalysisStatus): string {
    return analysisStatusLabels[status];
  }

  sentimentLabel(sentiment: Sentiment | null | undefined): string {
    return sentiment === null || sentiment === undefined ? '—' : sentimentLabels[sentiment];
  }

  priorityLabel(priority: Priority | null | undefined): string {
    return priority === null || priority === undefined ? '—' : priorityLabels[priority];
  }

  categoryLabel(category: ReviewCategory | null | undefined): string {
    return category === null || category === undefined ? '—' : categoryLabels[category];
  }

  sentimentBadgeClass(sentiment: Sentiment | null | undefined): string {
    return sentiment === 2 ? 'tag tag--negative' : sentiment === 0 ? 'tag tag--positive' : 'tag';
  }

  priorityBadgeClass(priority: Priority | null | undefined): string {
    return priority === 3 ? 'tag tag--critical' : priority === null || priority === undefined ? 'tag' : 'tag tag--priority';
  }

  private load(): void {
    this.loading.set(true);
    this.api.list(this.filters).subscribe({
      next: (reviews) => {
        this.reviews.set(reviews);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Не вдалося завантажити відгуки.');
        this.loading.set(false);
      },
    });
  }
}
