import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

import agent from "../api/agent"
import { useLocation } from "react-router";

export const useActivities = (id?:string) => {

  const queryClient = useQueryClient();
  const location = useLocation();


  const { data: activities, isPending } = useQuery({
    queryKey: ['activities'],
    queryFn: async () => {
      const response = await agent.get<Activity[]>('/activities')
      return response.data
    },
    staleTime:1000*60*5,
    enabled: !id && location.pathname === '/activities'
  });

  //web hook to fetch a single activity from API based on id
  const {data:activity, isLoading:isLoadingActivity} = useQuery({
      queryKey:['activity',id],
      queryFn: async () => {
        const response = await agent.get<Activity>(`/activities/${id}`)
        return response.data
      },
      staleTime:1000*60*5,enabled: !!id //if we have the id return true and then execute this hook
      
  })

  const updateActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      await agent.put('/activities', activity)
    }
    ,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['activities']
      })
    },
    onError: (error) => {
      console.error('Error updating activity(webooks folder):', error);
    }
  });

  const createActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      const response = await agent.post('/activities', activity)
      return response
    }
    ,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['activities']
      })
    },
    onError: (error) => {
      console.error('Error creating an activity:', error);
    }
  });

  const deleteActivity = useMutation({
    mutationFn: async (id: string) => {
      await agent.delete(`/activities/${id}`)
    }
    ,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['activities']
      })
    },
    onError: (error) => {
      console.error('Error updating activity:', error);
    }
  });

  return {
    activities,
    isPending,
    updateActivity,
    createActivity,
    deleteActivity,
    activity,
    isLoadingActivity
  }

}
