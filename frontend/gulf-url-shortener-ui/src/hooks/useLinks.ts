import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { linkApi } from "../services/linkApi";
import type { CreateLinkValues } from "../types/link";

const LINKS_QUERY_KEY = ["links"];

export function useLinks() {
  const queryClient = useQueryClient();
  const linksQuery = useQuery({
    queryKey: LINKS_QUERY_KEY,
    queryFn: linkApi.getAll,
    refetchOnWindowFocus: true,
  });
  const invalidateLinks = () =>
    queryClient.invalidateQueries({ queryKey: LINKS_QUERY_KEY });
  const createMutation = useMutation({
    mutationFn: (values: CreateLinkValues) => linkApi.create(values),
    onSuccess: invalidateLinks,
  });
  const disableMutation = useMutation({
    mutationFn: (code: string) => linkApi.disable(code),
    onSuccess: invalidateLinks,
  });
  const deleteMutation = useMutation({
    mutationFn: (code: string) => linkApi.remove(code),
    onSuccess: invalidateLinks,
  });

  return {
    links: linksQuery.data ?? [],
    loading: linksQuery.isLoading || linksQuery.isFetching,
    error: linksQuery.error,
    refresh: linksQuery.refetch,
    create: createMutation.mutateAsync,
    disable: disableMutation.mutateAsync,
    remove: deleteMutation.mutateAsync,
    isCreating: createMutation.isPending,
  };
}
