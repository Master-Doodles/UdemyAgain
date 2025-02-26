import { Person } from "@mui/icons-material";
import { Card, CardMedia, CardContent, Box, Typography, Chip, Divider } from "@mui/material";
import { Link } from "react-router";

type Props = {
    profile: Profile
}
export default function ProfileCard({profile}:Props) {

  return (
    <Link to={`/profiles/${profile.id}`} style={{ textDecoration: 'none' }}>
            <Card
                sx={{
                    borderRadius: 3, p: 3,
                    maxWidth: 250,
                    minHeight: 350,
                    height:'100%',
                    textDecoration: 'none'
                }}
                elevation={4}
            >
                <CardMedia 
                    component='img' 
                    src={profile?.imageUrl || '/images/user.png'} 
                    sx={{width: '100%', zIndex: 50}}
                    alt={profile.displayName + ' image'}
                />
                <CardContent>
                    <Box display='flex' flexDirection='column' alignItems='center' gap={1}>
                        <Typography variant="h5">{profile.displayName}</Typography>
                {profile.bio &&(
                    <Typography variant="body2" sx={{
                        textOverflow:'ellipsis',
                        overflow:'hidden',
                        whiteSpace:'nowrap',
                        width: '100%'
                    }}>
                        {profile.bio}
                    </Typography>
                )}



                        {profile.following && <Chip size="small" label='Following' 
                            color="secondary" variant="outlined" />}
                    </Box>
                </CardContent>
                <Divider sx={{mb: 2}} />
                <Box sx={{display: 'flex', alignItems: 'center', justifyContent: 'start'}}>
                    <Person />
                    <Typography sx={{ml: 1}}>{profile.followersCount}</Typography>
                </Box>
            </Card>
        </Link>
  )
}
