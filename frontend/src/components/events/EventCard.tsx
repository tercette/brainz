import type { CalendarEvent } from '../../types/event';
import { formatDateTime } from '../../utils/dateUtils';

interface EventCardProps {
  event: CalendarEvent;
}

export default function EventCard({ event }: EventCardProps) {
  return (
    <div
      className={`bg-white rounded-lg shadow p-4 border-l-4 ${
        event.isCancelled ? 'border-red-400 opacity-60' : 'border-indigo-500'
      }`}
    >
      <div className="flex justify-between items-start">
        <h3 className="text-sm font-semibold text-gray-900">
          {event.subject}
          {event.isCancelled && (
            <span className="ml-2 text-xs text-red-500 font-normal">Cancelado</span>
          )}
        </h3>
        {event.isAllDay && (
          <span className="text-xs bg-indigo-100 text-indigo-700 px-2 py-0.5 rounded">
            Dia inteiro
          </span>
        )}
      </div>

      <div className="mt-2 space-y-1">
        <p className="text-xs text-gray-500">
          {formatDateTime(event.startDateTime)} \u2014 {formatDateTime(event.endDateTime)}
        </p>

        {event.location && (
          <p className="text-xs text-gray-500">Local: {event.location}</p>
        )}

        {event.organizerName && (
          <p className="text-xs text-gray-500">Organizador: {event.organizerName}</p>
        )}

        {event.bodyPreview && (
          <p className="text-xs text-gray-400 mt-2 line-clamp-2">{event.bodyPreview}</p>
        )}
      </div>
    </div>
  );
}
