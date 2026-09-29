import { AnalysisStatus } from './analysis-status.model';
import { Priority } from './priority.model';
import { ReviewCategory } from './review-category.model';
import { Sentiment } from './sentiment.model';

export interface Review {
  id: string;
  authorName: string;
  email: string;
  text: string;
  createdAtUtc: string;
  analysisStatus: AnalysisStatus;
  sentiment?: Sentiment | null;
  priority?: Priority | null;
  category?: ReviewCategory | null;
  needsUrgentResponse?: boolean;
  summary?: string;
  aiDraftResponse?: string;
}
