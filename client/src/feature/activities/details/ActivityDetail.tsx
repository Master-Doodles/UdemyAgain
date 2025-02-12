import { Button, Card, CardContent, CardMedia, Typography } from "@mui/material";
import { useActivities } from "../../../lib/hooks/useActivities";

type Props = {
    selectedActivity: Activity;
    cancelSelectActivity: () => void;
    openForm: (id?: string) => void;
}

export default function ActivityDetails({selectedActivity,cancelSelectActivity,openForm}:Props) {
    const {activities}= useActivities();
    const activity = activities?.find(x=>x.id === selectedActivity.id);

    if(!activity) return <Typography variant="h4">Activity not found</Typography>;
    
    return (
        <Card sx={{borderRadius:3}}>
            <CardMedia component='img' src={`/images/categoryImages/${activity.category}.jpg`} />
            <CardContent>
                <Typography variant="h5">{activity.title}</Typography>
                <Typography variant="subtitle1" fontWeight='light'>{activity.date}</Typography>
                <Typography variant="body1">{activity.description}</Typography>
            </CardContent>
            <CardContent>
                <Button onClick={()=> openForm(activity.id)} color="primary">Edit</Button>
                <Button onClick={cancelSelectActivity} color="inherit">Cancel</Button>
            </CardContent>
        </Card>
    )
}
