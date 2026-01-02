import React from "react";
import { useQuery } from "@tanstack/react-query";
import { Link as RouterLink } from "react-router-dom";
import { Box, Container, Heading, Link, Stack, Text } from "@chakra-ui/react";
import { listCourses } from "../api/courses";

export default function CoursesPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ["courses"],
    queryFn: listCourses,
  });

  return (
    <Container maxW="2xl" py={10}>
      <Stack gap={6}>
        <Heading size="lg">Courses</Heading>

        {isLoading ? <Text>Loading…</Text> : null}
        {error ? <Text color="red.500">{error.message}</Text> : null}

        <Stack gap={3}>
          {(data || []).map((c) => (
            <Box key={c.id} borderWidth="1px" borderRadius="md" p={4}>
              <Stack gap={1}>
                <Link
                  as={RouterLink}
                  to={`/courses/${c.id}`}
                  fontWeight="semibold"
                >
                  {c.title}
                </Link>
                {c.description ? <Text>{c.description}</Text> : null}
                <Text fontSize="sm" opacity={0.8}>
                  Instructor: {c.instructorName}
                </Text>
              </Stack>
            </Box>
          ))}
        </Stack>
      </Stack>
    </Container>
  );
}
