import { AnalysisStatus } from '../models/review/analysis-status.model';
import { Priority } from '../models/review/priority.model';
import { ReviewCategory } from '../models/review/review-category.model';
import { Sentiment } from '../models/review/sentiment.model';

export const analysisStatusLabels: Record<AnalysisStatus, string> = {
  0: 'Очікує аналізу',
  1: 'Аналізується',
  2: 'Проаналізовано',
  3: 'Помилка аналізу',
};

export const sentimentLabels: Record<Sentiment, string> = {
  0: 'Позитивна',
  1: 'Нейтральна',
  2: 'Негативна',
};

export const priorityLabels: Record<Priority, string> = {
  0: 'Низький',
  1: 'Середній',
  2: 'Високий',
  3: 'Критичний',
};

export const categoryLabels: Record<ReviewCategory, string> = {
  0: 'Похвала',
  1: 'Запит функції',
  2: 'Помилка',
  3: 'Питання оплати',
  4: 'Підтримка',
  5: 'Інше',
};
