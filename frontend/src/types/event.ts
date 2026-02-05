export interface CalendarEvent {
  id: string;
  subject: string;
  bodyPreview?: string;
  startDateTime: string;
  endDateTime: string;
  location?: string;
  isAllDay: boolean;
  organizerName?: string;
  isCancelled: boolean;
}
