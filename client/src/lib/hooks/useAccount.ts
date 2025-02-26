import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { loginSchema } from "../schemas/loginSchema"
import agent from "../api/agent"
import { useLocation, useNavigate } from "react-router";
import { RegisterSchema } from "../schemas/registerSchema";
import { toast } from "react-toastify";

export const useAccount = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const location = useLocation();
    // Hook to handle user login
    const loginUser = useMutation({
        mutationFn: async (creds: loginSchema) => {
            // Sends a POST request to the '/login' endpoint with credentials, using cookies for authentication
            await agent.post('/login?useCookies=true', creds);
        },
        onSuccess: async () => { //refreshes the stale user-info
            await queryClient.invalidateQueries({
                queryKey: ['user']
            })

        }
    });

    const registerUser = useMutation({
        mutationFn: async (creds: RegisterSchema) => {
            await agent.post('/account/register', creds)

        }, onSuccess: () => {
            toast.success("Registration Was Successful");
            navigate('/login')
        },
        onError: () => {
            toast.success("Registration Failed");

        }
    })

    //logout the user
    const logoutUser = useMutation({
        mutationFn: async () => {
            await agent.post('/account/logout')
        },
        onSuccess: async () => {
            queryClient.removeQueries({ queryKey: ['user'] })
            queryClient.removeQueries({ queryKey: ['activities'] })
            navigate('/login')

        }
    })
    // Hook to fetch the current user's data
    const { data: currentUser, isLoading: loadingUserInfo } = useQuery({
        queryKey: ['user'],
        queryFn: async () => {
            // Sends a GET request to retrieve user info from the '/account/user-info' endpoint
            const response = await agent.get<User>('/account/user-info');
            // console.log('API Response:', response);
            // console.log('API Data:', response.data);
            // console.log('Current User:', { currentUser });
            return response.data;
        },
        enabled: !queryClient.getQueryData(['user']) && location.pathname !== '/register' //if the user object is not in the react query data we get it 

    })

    return {
        loginUser,
        currentUser,
        logoutUser,
        loadingUserInfo,
        registerUser
    }
}