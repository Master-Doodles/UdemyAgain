import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

import agent from "../api/agent"
import { useLocation } from "react-router";
import { useAccount } from "./useAccount";


export const useActivities = (id?: string) => {

  const queryClient = useQueryClient();
  const location = useLocation();
  const { currentUser } = useAccount();


  const { data: activities, isLoading } = useQuery({
    queryKey: ['activities'],
    queryFn: async () => {
      const response = await agent.get<Activity[]>('/activities')
      return response.data
    },
    // staleTime:1000*60*5,
    enabled: !id && location.pathname === '/activities' && !!currentUser,
    select: data => {
      return data.map(activity => {
        return {
          ...activity,
          isHost: currentUser?.id === activity.hostId,
          isGoing: activity.attendees.some(x => x.id === currentUser?.id)
        }
      })
    }
  });

  //web hook to fetch a single activity from API based on id
  const { data: activity, isLoading: isLoadingActivity } = useQuery({
    queryKey: ['activities', id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/activities/${id}`)
      return response.data
    },
    // staleTime:1000*60*5,
    enabled: !!id && !!currentUser,//if we have the id return true and then execute this hook
    select: data => {
      return {
        ...data,
        isHost: currentUser?.id === data.hostId,
        isGoing: data.attendees.some(x => x.id === currentUser?.id)
      }
    }
  })

  const updateActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      console.log("activity info: ",activity);
      await agent.put(`/activities/${activity.id}`, activity)
    }
    ,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['activities']
      })
    },
    onError: (error) => {
      console.error('Error updating activity(webhooks folder):', error);
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

  const updateAttendance = useMutation({
    mutationFn: async (id: string) => {
      // console.log('mutation id' + id)
      await agent.post(`/activities/${id}/attend`)

    },
    onMutate: async (activityId: string) => {//onMutate means before we go and try and make changes to the server

      await queryClient.cancelQueries({ queryKey: ['activities', activityId] }); //we cancel any ongoing queries

      const prevActivity = queryClient.getQueryData<Activity>(['activities', activityId]);//saves the previous qeury data

      queryClient.setQueryData<Activity>(['activities', activityId], oldActivity => {//updates the local cache with a modified version of the activity
        if (!oldActivity || !currentUser) { //if no activity exists or user is missing return the original data
          return oldActivity
        }

        const isHost = oldActivity.hostId === currentUser.id; //check to see if host
        const isAttending = oldActivity.attendees.some(x => x.id === currentUser.id);// check to see if current user is attending

        return {
          ...oldActivity,
          isCancelled: isHost ? !oldActivity.isCancelled : oldActivity.isCancelled, //if the user is host cancel / uncancel the event
          attendees: isAttending? isHost
              ? oldActivity.attendees : oldActivity.attendees.filter(x => x.id !== currentUser.id)// so we remove the user from the attendees list if they are not the host but are attending
            : [...oldActivity.attendees, { id: currentUser.id, displayName: currentUser.displayName, imageUrl: currentUser.imageUrl }] //
        }

      })
      return { prevActivity };//if any errors or no changes then rollback 

    },
    onError: (error, activityId, context) => {
      console.log('prevActivity' + context?.prevActivity);
      console.log(error);
      if (context?.prevActivity) {
        queryClient.setQueryData(['activities', activityId], context.prevActivity)
      }
    }
  })

  return {
    activities,
    isLoading,
    updateActivity,
    createActivity,
    deleteActivity,
    activity,
    isLoadingActivity,
    updateAttendance
  }

}
