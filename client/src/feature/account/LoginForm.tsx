import { zodResolver } from "@hookform/resolvers/zod";
import { useAccount } from "../../lib/hooks/useAccount";
import { loginSchema } from "../../lib/schemas/loginSchema";
import { useForm } from "react-hook-form";
import { Box, Button, Paper, Typography } from "@mui/material";
import { LockOpen } from "@mui/icons-material";
import TextInput from "../../app/shared/components/TextInput";
import { Link, useLocation, useNavigate } from "react-router";

export default function LoginForm() {
    // Extracts the loginUser function from the UseAccount hook (likely for handling authentication)
    const { loginUser } = useAccount();
    const navigate = useNavigate();
    const location = useLocation();
    // Initializes React Hook Form with Zod validation
    const { control, handleSubmit, formState: { isValid, isSubmitting } } = useForm<loginSchema>({
        mode: 'onTouched',// Runs validation when a field is touched
        resolver: zodResolver(loginSchema)// Uses Zod schema for form validation
    });

    const onSubmit = async (data: loginSchema) => {
        await loginUser.mutateAsync(data, {
            onSuccess: () => {
                navigate(location.state?.from || '/activities');
            }
        })
    }

    return (
        <Paper component='form' onSubmit={handleSubmit(onSubmit)} sx={{ display: 'flex', flexDirection: 'column', p: 3, gap: 3, maxWidth: 'md', mx: 'auto', borderRadius: 3 }}>
            <Box display='flex' alignItems='center' justifyContent='center' gap={3} color='secondary.main'>
                <LockOpen fontSize="large" />
                <Typography variant="h4">Sign in</Typography>
            </Box>
            <TextInput label='Email' control={control} name="email" />
            <TextInput label='Password' type='password' control={control} name="password" />
            <Button type='submit' disabled={!isValid || isSubmitting} variant="contained" size="large">
                Login
            </Button>
            <Typography sx={{textAlign:'center'}}>
                Don' have an account?
                <Typography sx={{ml:1}} component ={Link} to='/register' color="primary" >
                        Sign up
                </Typography>
            </Typography>
        </Paper>
    )
}
