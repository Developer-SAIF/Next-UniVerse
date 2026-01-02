import React, { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { useNavigate, Link as RouterLink } from "react-router-dom";
import {
  Box,
  Button,
  Container,
  Heading,
  Input,
  Link,
  Stack,
  Text,
} from "@chakra-ui/react";
import { register } from "../api/auth";
import { setAccessToken } from "../auth/token";

export default function RegisterPage() {
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const mutation = useMutation({
    mutationFn: () => register({ name, email, password }),
    onSuccess: (data) => {
      setAccessToken(data?.accessToken);
      navigate("/courses");
    },
    onError: (err) => {
      setError(err?.message || "Registration failed");
    },
  });

  return (
    <Container maxW="md" py={10}>
      <Stack gap={6}>
        <Heading size="lg">Register</Heading>

        <Box
          as="form"
          onSubmit={(e) => {
            e.preventDefault();
            setError("");
            mutation.mutate();
          }}
        >
          <Stack gap={4}>
            <Stack gap={1}>
              <Text fontSize="sm">Name</Text>
              <Input
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </Stack>

            <Stack gap={1}>
              <Text fontSize="sm">Email</Text>
              <Input
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                type="email"
                required
              />
            </Stack>

            <Stack gap={1}>
              <Text fontSize="sm">Password</Text>
              <Input
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                type="password"
                required
              />
            </Stack>

            {error ? <Text color="red.500">{error}</Text> : null}

            <Button type="submit" isLoading={mutation.isPending}>
              Create account
            </Button>

            <Text>
              Already have an account?{" "}
              <Link as={RouterLink} to="/login">
                Login
              </Link>
            </Text>
          </Stack>
        </Box>
      </Stack>
    </Container>
  );
}
