import { Grid2 } from "@mui/material";
import ActivityList from "./ActivityList";
import ActivitiesFilter from "./ActivitiesFilter";


export default function ActivityDashboard() {
    return (
        <Grid2 container spacing={3}>
            <Grid2 size={8}>
                <ActivityList/>
        
            </Grid2>
            <Grid2 size={4} 
            sx={{position:'sticky', top:95,
                alignSelf:'flex-start'
            }}>
                    <ActivitiesFilter/>
            </Grid2>
        </Grid2>
    )
}
