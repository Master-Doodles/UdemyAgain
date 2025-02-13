import { DateArg, format } from "date-fns";

export function formatDate(date:DateArg<Date>){
    return format(date,'d MMM yyyy h:mm a')
}