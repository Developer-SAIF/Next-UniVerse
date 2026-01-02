import React, { useMemo, useState } from "react";
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
import { Link as RouterLink } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getAccessToken, isInstructorToken } from "../auth/token";
import {
  createCourse,
  listMyCourses,
  publishCourse,
  unpublishCourse,
} from "../api/instructorCourses";
import {
  createMaterial,
  createModule,
  deleteMaterial,
  deleteModule,
  listModules,
  updateMaterial,
  updateModule,
} from "../api/instructorContent";

export default function InstructorDashboardPage() {
  const token = getAccessToken();
  const hasToken = Boolean(token);
  const isInstructor = isInstructorToken(token);
  const qc = useQueryClient();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [language, setLanguage] = useState("en");
  const [level, setLevel] = useState("Beginner");
  const [createError, setCreateError] = useState("");

  const [expandedCourseId, setExpandedCourseId] = useState(null);
  const [moduleTitle, setModuleTitle] = useState("");
  const [modulePosition, setModulePosition] = useState("");
  const [moduleDescription, setModuleDescription] = useState("");
  const [moduleError, setModuleError] = useState("");

  const [materialKind, setMaterialKind] = useState("Video");
  const [materialTitle, setMaterialTitle] = useState("");
  const [materialUrl, setMaterialUrl] = useState("");
  const [materialPosition, setMaterialPosition] = useState("");
  const [materialError, setMaterialError] = useState("");

  const [editingModuleId, setEditingModuleId] = useState(null);
  const [editModuleTitle, setEditModuleTitle] = useState("");
  const [editModulePosition, setEditModulePosition] = useState("");
  const [editModuleDescription, setEditModuleDescription] = useState("");

  const [editingMaterialId, setEditingMaterialId] = useState(null);
  const [editMaterialKind, setEditMaterialKind] = useState("Video");
  const [editMaterialTitle, setEditMaterialTitle] = useState("");
  const [editMaterialUrl, setEditMaterialUrl] = useState("");
  const [editMaterialPosition, setEditMaterialPosition] = useState("");

  const queryKey = useMemo(() => ["instructor", "courses"], []);
  const modulesQueryKey = useMemo(
    () => ["instructor", "course", expandedCourseId, "modules"],
    [expandedCourseId]
  );

  const coursesQuery = useQuery({
    queryKey,
    queryFn: listMyCourses,
    enabled: hasToken,
  });

  const createMutation = useMutation({
    mutationFn: () =>
      createCourse({
        title,
        description: description || null,
        language: language || null,
        level: level || null,
      }),
    onSuccess: async () => {
      setTitle("");
      setDescription("");
      setCreateError("");
      await qc.invalidateQueries({ queryKey });
    },
    onError: (err) => {
      setCreateError(err?.message || "Create course failed");
    },
  });

  const publishMutation = useMutation({
    mutationFn: ({ id, publish }) =>
      publish ? publishCourse(id) : unpublishCourse(id),
    onSuccess: async () => {
      await qc.invalidateQueries({ queryKey });
    },
  });

  const modulesQuery = useQuery({
    queryKey: modulesQueryKey,
    queryFn: () => listModules(expandedCourseId),
    enabled: hasToken && Boolean(expandedCourseId),
  });

  const createModuleMutation = useMutation({
    mutationFn: () =>
      createModule(expandedCourseId, {
        title: moduleTitle,
        position: modulePosition === "" ? null : Number(modulePosition),
        description: moduleDescription || null,
      }),
    onSuccess: async () => {
      setModuleTitle("");
      setModulePosition("");
      setModuleDescription("");
      setModuleError("");
      await qc.invalidateQueries({ queryKey: modulesQueryKey });
    },
    onError: (err) => {
      setModuleError(err?.message || "Create module failed");
    },
  });

  const updateModuleMutation = useMutation({
    mutationFn: () =>
      updateModule(editingModuleId, {
        title: editModuleTitle,
        position: editModulePosition === "" ? null : Number(editModulePosition),
        description: editModuleDescription || null,
      }),
    onSuccess: async () => {
      setEditingModuleId(null);
      await qc.invalidateQueries({ queryKey: modulesQueryKey });
    },
  });

  const deleteModuleMutation = useMutation({
    mutationFn: (moduleId) => deleteModule(moduleId),
    onSuccess: async () => {
      await qc.invalidateQueries({ queryKey: modulesQueryKey });
    },
  });

  const createMaterialMutation = useMutation({
    mutationFn: (moduleId) =>
      createMaterial(moduleId, {
        kind: materialKind,
        title: materialTitle,
        description: null,
        storageUrl: materialUrl || null,
        mimeType: null,
        sizeBytes: null,
        requiresAuth: false,
        position: materialPosition === "" ? null : Number(materialPosition),
      }),
    onSuccess: async () => {
      setMaterialTitle("");
      setMaterialUrl("");
      setMaterialPosition("");
      setMaterialError("");
      await qc.invalidateQueries({ queryKey: modulesQueryKey });
    },
    onError: (err) => {
      setMaterialError(err?.message || "Create material failed");
    },
  });

  const updateMaterialMutation = useMutation({
    mutationFn: () =>
      updateMaterial(editingMaterialId, {
        kind: editMaterialKind,
        title: editMaterialTitle,
        description: null,
        storageUrl: editMaterialUrl || null,
        mimeType: null,
        sizeBytes: null,
        requiresAuth: false,
        position:
          editMaterialPosition === "" ? null : Number(editMaterialPosition),
      }),
    onSuccess: async () => {
      setEditingMaterialId(null);
      await qc.invalidateQueries({ queryKey: modulesQueryKey });
    },
  });

  const deleteMaterialMutation = useMutation({
    mutationFn: (materialId) => deleteMaterial(materialId),
    onSuccess: async () => {
      await qc.invalidateQueries({ queryKey: modulesQueryKey });
    },
  });

  if (!hasToken) {
    return (
      <Container maxW="2xl" py={10}>
        <Stack gap={4}>
          <Heading size="lg">Instructor Dashboard</Heading>
          <Text>
            You need to be logged in as an instructor to access this page.
          </Text>
          <Text>
            Go to{" "}
            <Link as={RouterLink} to="/login">
              Login
            </Link>
            .
          </Text>
        </Stack>
      </Container>
    );
  }

  if (!isInstructor) {
    return (
      <Container maxW="2xl" py={10}>
        <Stack gap={4}>
          <Heading size="lg">Instructor Dashboard</Heading>
          <Text>You are logged in, but your account is not an instructor.</Text>
          <Text>
            Go back to{" "}
            <Link as={RouterLink} to="/courses">
              Courses
            </Link>
            .
          </Text>
        </Stack>
      </Container>
    );
  }

  return (
    <Container maxW="2xl" py={10}>
      <Stack gap={8}>
        <Heading size="lg">Instructor Dashboard</Heading>

        <Box borderWidth="1px" borderRadius="md" p={4}>
          <Stack gap={3}>
            <Heading size="md">Create course</Heading>

            <Stack gap={1}>
              <Text fontSize="sm">Title</Text>
              <Input
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="e.g., Intro to Databases"
              />
            </Stack>

            <Stack gap={1}>
              <Text fontSize="sm">Description</Text>
              <Input
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Short summary"
              />
            </Stack>

            <Stack gap={1}>
              <Text fontSize="sm">Language</Text>
              <Input
                value={language}
                onChange={(e) => setLanguage(e.target.value)}
                placeholder="en"
              />
            </Stack>

            <Stack gap={1}>
              <Text fontSize="sm">Level</Text>
              <Input
                value={level}
                onChange={(e) => setLevel(e.target.value)}
                placeholder="Beginner | Intermediate | Advanced"
              />
            </Stack>

            {createError ? <Text color="red.500">{createError}</Text> : null}

            <Button
              onClick={() => {
                setCreateError("");
                createMutation.mutate();
              }}
              isDisabled={!title.trim()}
              isLoading={createMutation.isPending}
            >
              Create
            </Button>

            <Text fontSize="sm" opacity={0.8}>
              Note: course starts as draft (unpublished).
            </Text>
          </Stack>
        </Box>

        <Box>
          <Stack gap={4}>
            <Heading size="md">My courses</Heading>

            {coursesQuery.isLoading ? <Text>Loading…</Text> : null}
            {coursesQuery.error ? (
              <Text color="red.500">{coursesQuery.error.message}</Text>
            ) : null}

            <Stack gap={3}>
              {(coursesQuery.data || []).map((c) => (
                <Box key={c.id} borderWidth="1px" borderRadius="md" p={4}>
                  <Stack gap={2}>
                    <Text fontWeight="semibold">{c.title}</Text>
                    {c.description ? <Text>{c.description}</Text> : null}
                    <Text fontSize="sm" opacity={0.8}>
                      Status: {c.isPublished ? "Published" : "Draft"}
                    </Text>

                    <Button
                      size="sm"
                      onClick={() =>
                        publishMutation.mutate({
                          id: c.id,
                          publish: !c.isPublished,
                        })
                      }
                      isLoading={publishMutation.isPending}
                    >
                      {c.isPublished ? "Unpublish" : "Publish"}
                    </Button>

                    <Text fontSize="sm" opacity={0.8}>
                      Public view:{" "}
                      <Link as={RouterLink} to={`/courses/${c.id}`}>
                        /courses/{c.id}
                      </Link>
                    </Text>

                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => {
                        const next = expandedCourseId === c.id ? null : c.id;
                        setExpandedCourseId(next);
                        setModuleError("");
                        setMaterialError("");
                        setEditingModuleId(null);
                        setEditingMaterialId(null);
                      }}
                    >
                      {expandedCourseId === c.id
                        ? "Hide modules"
                        : "Manage modules"}
                    </Button>

                    {expandedCourseId === c.id ? (
                      <Box borderWidth="1px" borderRadius="md" p={3}>
                        <Stack gap={4}>
                          <Heading size="sm">Modules</Heading>

                          {modulesQuery.isLoading ? (
                            <Text>Loading…</Text>
                          ) : null}
                          {modulesQuery.error ? (
                            <Text color="red.500">
                              {modulesQuery.error.message}
                            </Text>
                          ) : null}

                          <Box borderWidth="1px" borderRadius="md" p={3}>
                            <Stack gap={2}>
                              <Text fontWeight="semibold">Add module</Text>
                              <Stack gap={1}>
                                <Text fontSize="sm">Title</Text>
                                <Input
                                  value={moduleTitle}
                                  onChange={(e) =>
                                    setModuleTitle(e.target.value)
                                  }
                                />
                              </Stack>
                              <Stack gap={1}>
                                <Text fontSize="sm">Position (optional)</Text>
                                <Input
                                  value={modulePosition}
                                  onChange={(e) =>
                                    setModulePosition(e.target.value)
                                  }
                                  placeholder="1"
                                />
                              </Stack>
                              <Stack gap={1}>
                                <Text fontSize="sm">
                                  Description (optional)
                                </Text>
                                <Input
                                  value={moduleDescription}
                                  onChange={(e) =>
                                    setModuleDescription(e.target.value)
                                  }
                                />
                              </Stack>
                              {moduleError ? (
                                <Text color="red.500">{moduleError}</Text>
                              ) : null}
                              <Button
                                size="sm"
                                onClick={() => {
                                  setModuleError("");
                                  createModuleMutation.mutate();
                                }}
                                isDisabled={!moduleTitle.trim()}
                                isLoading={createModuleMutation.isPending}
                              >
                                Create module
                              </Button>
                            </Stack>
                          </Box>

                          <Stack gap={3}>
                            {(modulesQuery.data || []).map((m) => (
                              <Box
                                key={m.id}
                                borderWidth="1px"
                                borderRadius="md"
                                p={3}
                              >
                                <Stack gap={2}>
                                  <Text fontWeight="semibold">
                                    {m.title}
                                    {m.position != null
                                      ? ` (pos ${m.position})`
                                      : ""}
                                  </Text>

                                  {editingModuleId === m.id ? (
                                    <Box
                                      borderWidth="1px"
                                      borderRadius="md"
                                      p={3}
                                    >
                                      <Stack gap={2}>
                                        <Text fontWeight="semibold">
                                          Edit module
                                        </Text>
                                        <Stack gap={1}>
                                          <Text fontSize="sm">Title</Text>
                                          <Input
                                            value={editModuleTitle}
                                            onChange={(e) =>
                                              setEditModuleTitle(e.target.value)
                                            }
                                          />
                                        </Stack>
                                        <Stack gap={1}>
                                          <Text fontSize="sm">Position</Text>
                                          <Input
                                            value={editModulePosition}
                                            onChange={(e) =>
                                              setEditModulePosition(
                                                e.target.value
                                              )
                                            }
                                            placeholder="1"
                                          />
                                        </Stack>
                                        <Stack gap={1}>
                                          <Text fontSize="sm">Description</Text>
                                          <Input
                                            value={editModuleDescription}
                                            onChange={(e) =>
                                              setEditModuleDescription(
                                                e.target.value
                                              )
                                            }
                                          />
                                        </Stack>
                                        <Button
                                          size="sm"
                                          onClick={() =>
                                            updateModuleMutation.mutate()
                                          }
                                          isLoading={
                                            updateModuleMutation.isPending
                                          }
                                          isDisabled={!editModuleTitle.trim()}
                                        >
                                          Save module
                                        </Button>
                                        <Button
                                          size="sm"
                                          variant="outline"
                                          onClick={() =>
                                            setEditingModuleId(null)
                                          }
                                        >
                                          Cancel
                                        </Button>
                                      </Stack>
                                    </Box>
                                  ) : (
                                    <Stack
                                      direction={{ base: "column", md: "row" }}
                                      gap={2}
                                    >
                                      <Button
                                        size="sm"
                                        variant="outline"
                                        onClick={() => {
                                          setEditingModuleId(m.id);
                                          setEditModuleTitle(m.title || "");
                                          setEditModulePosition(
                                            m.position == null
                                              ? ""
                                              : String(m.position)
                                          );
                                          setEditModuleDescription(
                                            m.description || ""
                                          );
                                        }}
                                      >
                                        Edit
                                      </Button>
                                      <Button
                                        size="sm"
                                        colorScheme="red"
                                        variant="outline"
                                        onClick={() =>
                                          deleteModuleMutation.mutate(m.id)
                                        }
                                        isLoading={
                                          deleteModuleMutation.isPending
                                        }
                                      >
                                        Delete
                                      </Button>
                                    </Stack>
                                  )}

                                  <Box
                                    borderWidth="1px"
                                    borderRadius="md"
                                    p={3}
                                  >
                                    <Stack gap={2}>
                                      <Text fontWeight="semibold">
                                        Add material
                                      </Text>
                                      <Text fontSize="sm" opacity={0.8}>
                                        Kind values: Video, Pdf, Slide, Html,
                                        Audio, Other
                                      </Text>
                                      <Stack gap={1}>
                                        <Text fontSize="sm">Kind</Text>
                                        <Input
                                          value={materialKind}
                                          onChange={(e) =>
                                            setMaterialKind(e.target.value)
                                          }
                                          placeholder="Video"
                                        />
                                      </Stack>
                                      <Stack gap={1}>
                                        <Text fontSize="sm">Title</Text>
                                        <Input
                                          value={materialTitle}
                                          onChange={(e) =>
                                            setMaterialTitle(e.target.value)
                                          }
                                        />
                                      </Stack>
                                      <Stack gap={1}>
                                        <Text fontSize="sm">
                                          URL (optional)
                                        </Text>
                                        <Input
                                          value={materialUrl}
                                          onChange={(e) =>
                                            setMaterialUrl(e.target.value)
                                          }
                                          placeholder="https://..."
                                        />
                                      </Stack>
                                      <Stack gap={1}>
                                        <Text fontSize="sm">
                                          Position (optional)
                                        </Text>
                                        <Input
                                          value={materialPosition}
                                          onChange={(e) =>
                                            setMaterialPosition(e.target.value)
                                          }
                                          placeholder="1"
                                        />
                                      </Stack>
                                      {materialError ? (
                                        <Text color="red.500">
                                          {materialError}
                                        </Text>
                                      ) : null}
                                      <Button
                                        size="sm"
                                        onClick={() => {
                                          setMaterialError("");
                                          createMaterialMutation.mutate(m.id);
                                        }}
                                        isDisabled={!materialTitle.trim()}
                                        isLoading={
                                          createMaterialMutation.isPending
                                        }
                                      >
                                        Add material
                                      </Button>
                                    </Stack>
                                  </Box>

                                  <Stack gap={2}>
                                    <Text fontWeight="semibold">Materials</Text>
                                    {(m.materials || []).length === 0 ? (
                                      <Text fontSize="sm">
                                        No materials yet.
                                      </Text>
                                    ) : null}
                                    {(m.materials || []).map((mat) => (
                                      <Box
                                        key={mat.id}
                                        borderWidth="1px"
                                        borderRadius="md"
                                        p={3}
                                      >
                                        <Stack gap={2}>
                                          <Text>
                                            {mat.title} ({mat.kind})
                                            {mat.position != null
                                              ? ` (pos ${mat.position})`
                                              : ""}
                                          </Text>
                                          {mat.storageUrl ? (
                                            <Link
                                              href={mat.storageUrl}
                                              target="_blank"
                                              rel="noreferrer"
                                            >
                                              {mat.storageUrl}
                                            </Link>
                                          ) : null}

                                          {editingMaterialId === mat.id ? (
                                            <Box
                                              borderWidth="1px"
                                              borderRadius="md"
                                              p={3}
                                            >
                                              <Stack gap={2}>
                                                <Text fontWeight="semibold">
                                                  Edit material
                                                </Text>
                                                <Stack gap={1}>
                                                  <Text fontSize="sm">
                                                    Kind
                                                  </Text>
                                                  <Input
                                                    value={editMaterialKind}
                                                    onChange={(e) =>
                                                      setEditMaterialKind(
                                                        e.target.value
                                                      )
                                                    }
                                                  />
                                                </Stack>
                                                <Stack gap={1}>
                                                  <Text fontSize="sm">
                                                    Title
                                                  </Text>
                                                  <Input
                                                    value={editMaterialTitle}
                                                    onChange={(e) =>
                                                      setEditMaterialTitle(
                                                        e.target.value
                                                      )
                                                    }
                                                  />
                                                </Stack>
                                                <Stack gap={1}>
                                                  <Text fontSize="sm">URL</Text>
                                                  <Input
                                                    value={editMaterialUrl}
                                                    onChange={(e) =>
                                                      setEditMaterialUrl(
                                                        e.target.value
                                                      )
                                                    }
                                                  />
                                                </Stack>
                                                <Stack gap={1}>
                                                  <Text fontSize="sm">
                                                    Position
                                                  </Text>
                                                  <Input
                                                    value={editMaterialPosition}
                                                    onChange={(e) =>
                                                      setEditMaterialPosition(
                                                        e.target.value
                                                      )
                                                    }
                                                  />
                                                </Stack>
                                                <Button
                                                  size="sm"
                                                  onClick={() =>
                                                    updateMaterialMutation.mutate()
                                                  }
                                                  isLoading={
                                                    updateMaterialMutation.isPending
                                                  }
                                                  isDisabled={
                                                    !editMaterialTitle.trim()
                                                  }
                                                >
                                                  Save material
                                                </Button>
                                                <Button
                                                  size="sm"
                                                  variant="outline"
                                                  onClick={() =>
                                                    setEditingMaterialId(null)
                                                  }
                                                >
                                                  Cancel
                                                </Button>
                                              </Stack>
                                            </Box>
                                          ) : (
                                            <Stack
                                              direction={{
                                                base: "column",
                                                md: "row",
                                              }}
                                              gap={2}
                                            >
                                              <Button
                                                size="sm"
                                                variant="outline"
                                                onClick={() => {
                                                  setEditingMaterialId(mat.id);
                                                  setEditMaterialKind(
                                                    mat.kind || "Video"
                                                  );
                                                  setEditMaterialTitle(
                                                    mat.title || ""
                                                  );
                                                  setEditMaterialUrl(
                                                    mat.storageUrl || ""
                                                  );
                                                  setEditMaterialPosition(
                                                    mat.position == null
                                                      ? ""
                                                      : String(mat.position)
                                                  );
                                                }}
                                              >
                                                Edit
                                              </Button>
                                              <Button
                                                size="sm"
                                                colorScheme="red"
                                                variant="outline"
                                                onClick={() =>
                                                  deleteMaterialMutation.mutate(
                                                    mat.id
                                                  )
                                                }
                                                isLoading={
                                                  deleteMaterialMutation.isPending
                                                }
                                              >
                                                Delete
                                              </Button>
                                            </Stack>
                                          )}
                                        </Stack>
                                      </Box>
                                    ))}
                                  </Stack>
                                </Stack>
                              </Box>
                            ))}
                          </Stack>
                        </Stack>
                      </Box>
                    ) : null}
                  </Stack>
                </Box>
              ))}
            </Stack>
          </Stack>
        </Box>
      </Stack>
    </Container>
  );
}
