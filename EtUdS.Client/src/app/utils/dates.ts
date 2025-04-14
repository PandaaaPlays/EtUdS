export function formatDate(dateString: string | Date, time: boolean): string {
  const date = new Date(dateString);

  if(time)
    return date.getFullYear()
      + "-" + ((date.getMonth() % 12) + 1).toString().padStart(2, '0')
      + "-" + date.getDate().toString().padStart(2, '0')
      + " (" + date.getHours().toString().padStart(2, '0')
      + ":" + date.getMinutes().toString().padStart(2, '0') + ")";
  else
    return date.getFullYear()
      + "-" + ((date.getMonth() % 12) + 1).toString().padStart(2, '0')
      + "-" + date.getDate().toString().padStart(2, '0');
}

export function formatTime(dateString: string): string {
  const date = new Date(dateString);
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${hours}:${minutes}`;
}

export function getMonthAbbreviation(month: number): string {
  const monthsAbbreviation = [
    "janv.", "févr.", "mars", "avr.", "mai", "juin", "juil.", "août", "sept.", "oct.", "nov.", "déc."
  ];
  return monthsAbbreviation[month];
}

export function getStartAndEndOfWeek(dateString: string): { startDate: string; endDate: string } {
  const date = getDate(dateString);

  const currentWeekday = date.getDay();
  const startOfWeek = new Date(date);
  startOfWeek.setDate(date.getDate() - currentWeekday);
  const endOfWeek = new Date(startOfWeek);
  endOfWeek.setDate(startOfWeek.getDate() + 6);

  return {
    startDate: formatDate(startOfWeek, false),
    endDate: formatDate(endOfWeek, false),
  };
}

export function getDate(dateString: string) {
  const [year, month, day] = dateString.split('-').map(Number);
  return new Date(year, month - 1, day);
}
