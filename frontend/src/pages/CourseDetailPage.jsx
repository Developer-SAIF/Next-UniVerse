import React from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Link as RouterLink, useParams } from "react-router-dom";
import {
  Box,
  Button,
  Container,
  Heading,
  Link,
  Stack,
  Text,
} from "@chakra-ui/react";
import { getCourse } from "../api/courses";
import { enroll } from "../api/enrollments";
import { getAccessToken } from "../auth/token";

export default function CourseDetailPage() {
  const { id } = useParams();
  const courseId = Number(id);
  const hasToken = Boolean(getAccessToken());

  const { data, isLoading, error } = useQuery({
    queryKey: ["course", courseId],
    queryFn: () => getCourse(courseId),
    enabled: Number.isFinite(courseId),
  });

  const enrollMutation = useMutation({
    mutationFn: () => enroll(courseId),
  });

  return (
    <Container maxW="2xl" py={10}>
      <Stack gap={6}>
        <Link as={RouterLink} to="/courses">
          ← Back to courses
        </Link>

        {isLoading ? <Text>Loading…</Text> : null}
        {error ? <Text color="red.500">{error.message}</Text> : null}

        {data ? (
          <>
            <Stack gap={2}>
              <Heading size="lg">{data.title}</Heading>
              {data.description ? <Text>{data.description}</Text> : null}
              <Text fontSize="sm" opacity={0.8}>
                Instructor: {data.instructorName}
              </Text>
            </Stack>

            <Box>
              <Button
                onClick={() => enrollMutation.mutate()}
                isDisabled={!hasToken}
                isLoading={enrollMutation.isPending}
              >
                Enroll
              </Button>
              {!hasToken ? (
                <Text fontSize="sm" opacity={0.8} mt={2}>
                  Please{" "}
                  <Link as={RouterLink} to="/login">
                    login
                  </Link>{" "}
                  to enroll.
                </Text>
              ) : null}
              {enrollMutation.isError ? (
                <Text color="red.500" mt={2}>
                  {enrollMutation.error?.message || "Enroll failed"}
                </Text>
              ) : null}
              {enrollMutation.isSuccess ? (
                <Text color="green.600" mt={2}>
                  Enrolled.
                </Text>
              ) : null}
            </Box>

            <Stack gap={4}>
              <Heading size="md">Modules</Heading>
              {(data.modules || []).length === 0 ? (
                <Text>No modules yet.</Text>
              ) : null}
              {(data.modules || []).map((m) => (
                <Box key={m.id} borderWidth="1px" borderRadius="md" p={4}>
                  <Stack gap={2}>
                    <Text fontWeight="semibold">{m.title}</Text>
                    {m.description ? <Text>{m.description}</Text> : null}

                    <Stack gap={1}>
                      {(m.materials || []).map((mat) => (
                        <Text key={mat.id} fontSize="sm" opacity={0.85}>
                          • {mat.title} ({mat.kind})
                        </Text>
                      ))}
                    </Stack>
                  </Stack>
                </Box>
              ))}
            </Stack>
          </>
        ) : null}
      </Stack>
    </Container>
  );
}
