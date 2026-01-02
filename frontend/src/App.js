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
import InstructorDashboardPage from "./pages/InstructorDashboardPage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import {
  clearAccessToken,
  getAccessToken,
  isInstructorToken,
} from "./auth/token";

function Header() {
  const navigate = useNavigate();
  const token = getAccessToken();
  const hasToken = Boolean(token);
  const isInstructor = isInstructorToken(token);

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

          {isInstructor ? (
            <Link as={RouterLink} to="/instructor">
              Instructor
            </Link>
          ) : null}

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
  const token = getAccessToken();
  const isInstructor = isInstructorToken(token);

  return (
    <Box minH="100vh">
      <Header />
      <Routes>
        <Route path="/" element={<Navigate to="/courses" replace />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/courses" element={<CoursesPage />} />
        <Route path="/courses/:id" element={<CourseDetailPage />} />
        <Route
          path="/instructor"
          element={
            isInstructor ? (
              <InstructorDashboardPage />
            ) : (
              <Navigate to="/courses" replace />
            )
          }
        />
        <Route path="*" element={<Navigate to="/courses" replace />} />
      </Routes>
    </Box>
  );
}

export default App;
