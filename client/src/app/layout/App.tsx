import { Box, Container, CssBaseline } from "@mui/material"
import NavBar from "./NavBar"
import { Outlet, ScrollRestoration, useLocation } from "react-router"
import HomePage from "../../feature/home/HomePage";

function App() {

  const location = useLocation();

  return (
    <Box sx={{
      backgroundColor: 'rgba(59, 89, 152, 0.5)',
      backdropFilter: 'blur(8px)',

      color: 'white',
      minHeight: '100vh'
    }}>
      <ScrollRestoration />
      <CssBaseline />
      {location.pathname === '/' ? <HomePage /> : (
        <>
          <NavBar />
          <Container maxWidth="xl" sx={{ mt: 3,pb: 3 }}>
            <Outlet />
            
          </Container>
        </>
      )}

    </Box>
  )
}

export default App
