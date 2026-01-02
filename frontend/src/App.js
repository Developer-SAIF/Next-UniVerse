import React from "react";
import {
  Routes,
  Route,
  Navigate,
  Link as RouterLink,
  useNavigate,
} from "react-router-dom";
import {
  Box,
  Button,
  Container,
  Flex,
  Heading,
  Link,
  Spacer,
} from "@chakra-ui/react";
import CoursesPage from "./pages/CoursesPage";
import CourseDetailPage from "./pages/CourseDetailPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { clearAccessToken, getAccessToken } from "./auth/token";

function Header() {
  const navigate = useNavigate();
  const hasToken = Boolean(getAccessToken());

  return (
    <Box borderBottomWidth="1px" py={3}>
      <Container maxW="2xl">
        <Flex align="center" gap={3}>
          <Heading size="md">
            <Link as={RouterLink} to="/courses">
              Next UniVerse
            </Link>
          </Heading>
          <Spacer />

          <Link as={RouterLink} to="/courses">
            Courses
          </Link>

          {hasToken ? (
            <Button
              size="sm"
              onClick={() => {
                clearAccessToken();
                navigate("/login");
              }}
            >
              Logout
            </Button>
          ) : (
            <>
              <Button size="sm" as={RouterLink} to="/login">
                Login
              </Button>
              <Button
                size="sm"
                variant="outline"
                as={RouterLink}
                to="/register"
              >
                Register
              </Button>
            </>
          )}
        </Flex>
      </Container>
    </Box>
  );
}

function App() {
  return (
    <Box minH="100vh">
      <Header />
      <Routes>
        <Route path="/" element={<Navigate to="/courses" replace />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/courses" element={<CoursesPage />} />
        <Route path="/courses/:id" element={<CourseDetailPage />} />
        <Route path="*" element={<Navigate to="/courses" replace />} />
      </Routes>
    </Box>
  );
}

export default App;
