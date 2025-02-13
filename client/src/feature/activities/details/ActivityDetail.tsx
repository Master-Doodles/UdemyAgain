import { Button, Card, CardContent, CardMedia, Typography } from "@mui/material";
import { Link, useNavigate, useParams } from "react-router";
import { useActivities } from "../../../lib/hooks/useActivities";




export default function ActivityDetails() {
    const navigate = useNavigate();
    const {id} = useParams(); //get the id from the url
    const {activity,isLoadingActivity}= useActivities(id); //get the activity based on the id
    
    if(isLoadingActivity) return <Typography>Loading Activity...</Typography>;

    if(!activity) return <Typography>Activity not found</Typography>;
    
    return (
        <Card sx={{borderRadius:3}}>
            <CardMedia component='img' src={`/images/categoryImages/${activity.category}.jpg`} />
            <CardContent>
                <Typography variant="h5">{activity.title}</Typography>
                <Typography variant="subtitle1" fontWeight='light'>{activity.date}</Typography>
                <Typography variant="body1">{activity.description}</Typography>
            </CardContent>
            <CardContent>

                <Button  component={Link} to={`/manage/${activity.id}`} color="primary">Edit</Button>
                <Button onClick={()=> navigate('/activities')} color="inherit">Cancel</Button>
            </CardContent>
        </Card>
    )
}
