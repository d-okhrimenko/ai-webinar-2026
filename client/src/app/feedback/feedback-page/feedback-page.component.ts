import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReviewsApi } from '../../core/reviews';

@Component({
  selector: 'app-feedback-page',
  imports: [ReactiveFormsModule],
  templateUrl: './feedback-page.component.html',
  styleUrl: './feedback-page.component.css',
})
export class FeedbackPage {
  private readonly api = inject(ReviewsApi);

  readonly saving = signal(false);
  readonly submitted = signal(false);
  readonly error = signal('');

  readonly form = new FormGroup({
    authorName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(150)],
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    text: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(3), Validators.maxLength(4000)],
    }),
  });

  submit(): void {
    if (this.form.invalid) return;

    this.saving.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.submitted.set(true);
        this.form.reset();
        this.saving.set(false);
      },
      error: () => {
        this.error.set('Не вдалося надіслати відгук. Спробуйте ще раз.');
        this.saving.set(false);
      },
    });
  }
}
