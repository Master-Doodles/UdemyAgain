import { Box, Button, Paper, TextField, Typography } from "@mui/material";
import { FormEvent } from "react";

type Props = {
    closeForm: () => void;
    activity: Activity | undefined;
    submitForm: (activity: Activity) => void;
}

export default function ActivityForm({ closeForm, activity,submitForm }: Props) {

    const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const data:{[key:string]:FormDataEntryValue} = {}
        formData.forEach((value, key) => {
            data[key] = value;
        })
        if(activity) data.id = activity.id;
        submitForm(data as unknown as Activity);
    }

    return (
        <Paper sx={{ borderRadiu: 3, padding: 2 }}>
            <Typography variant="h6" gutterBottom color="primary">
                Create Activity
            </Typography>
            <Box component="form" onSubmit={handleSubmit} display='flex' flexDirection='column' gap={3}>
                <TextField name="title" label="Title" defaultValue={activity?.title} />
                <TextField name="description" label="Description" multiline rows={3} defaultValue={activity?.description} />
                <TextField name="category" label="Category" defaultValue={activity?.category} />
                <TextField name="date" label="Date" type="date" defaultValue={activity?.date} />
                <TextField name="city" label="City" defaultValue={activity?.city} />
                <TextField name="venue" label="Venue" defaultValue={activity?.venue} />
                <Box display='flex' justifyContent='end' gap={3}>
                    <Button onClick={closeForm} color="success">Cancel</Button>
                    <Button type="submit" color="primary" variant="contained">Submit</Button>
                </Box>

            </Box>

        </Paper>
    )
}
