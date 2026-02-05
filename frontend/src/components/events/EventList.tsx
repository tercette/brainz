import type { CalendarEvent } from '../../types/event';
import EventCard from './EventCard';

interface EventListProps {
  events: CalendarEvent[];
}

export default function EventList({ events }: EventListProps) {
  if (events.length === 0) {
    return (
      <p className="text-center text-gray-500 py-8">Nenhum evento encontrado.</p>
    );
  }

  return (
    <div className="grid gap-3">
      {events.map((event) => (
        <EventCard key={event.id} event={event} />
      ))}
    </div>
  );
}
