import { createBrowserRouter } from "react-router";
import App from "../layout/App";
import HomePage from "../../feature/activities/home/HomePage";
import ActivityDashboard from "../../feature/activities/dashboard/ActivityDashboard";
import ActivityForm from "../../feature/activities/form/ActivityForm";
import ActivityDetails from "../../feature/activities/details/ActivityDetail";

export const router = createBrowserRouter([
    {
        path:'/',
        element: <App />,
        children:[
            {path:'', element: <HomePage />},
            {path:'activities', element: <ActivityDashboard />},
            {path:'activities/:id', element: <ActivityDetails />},
            {path:'createActivity', element: <ActivityForm key='create' />},
            {path:'manage/:id', element: <ActivityForm />},

        ]
    }
]
)