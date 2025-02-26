import { useLocalObservable } from "mobx-react-lite"
import {HubConnection, HubConnectionBuilder, HubConnectionState} from '@microsoft/signalr';
import { useEffect, useRef } from "react";
import { runInAction } from "mobx";

export const useComments = (activityId?: string) => {
    const created = useRef(false);
    const commentStore = useLocalObservable(() => ({
        comments: [] as ChatComment[],
        hubConnection: null as HubConnection | null,

        createHubConnection(activityId: string) {
            if (!activityId) return;//if we have no activityID dont connect comments

            this.hubConnection = new HubConnectionBuilder() // this sets up the connection to the SignalR hub
                .withUrl(`${import.meta.env.VITE_COMMENT_URL}?activityId=${activityId}`, {// it includes activityId as a query parameter, ussing wbesocket
                                                                                            // more information is needed on websockets for a beter understanding 
                    withCredentials: true // with creds incase it becomes important for us when we want to use it for deleting or editing comments or adding coments
                })
                .withAutomaticReconnect()// this will reconnect automatically if it disconnects
                .build();

            this.hubConnection.start().catch(error =>  // this starts the websocket server
                console.log('Error establishing connection: ', error));

            this.hubConnection.on('LoadComments', comments => { //this listens for the LoadComments event from the server
                runInAction(() => { //ussing mobx here to track the changes made to the state  comments
                    this.comments = comments
                })
            });

            this.hubConnection.on('ReceiveComment', comment => {// While this listens for the ReceiveComment event, I.E. a new comment being  made 
                runInAction(() => {//ussing mobx here to track the changest made to bellow
                    this.comments.unshift(comment);// this adds a new comment to the start of the comments array
                })
            })
        },

        stopHubConnection() { //this will stop the connection between the client and the hub, when the component is umounted or the activityId changes
            if (this.hubConnection?.state === HubConnectionState.Connected) {//if noconnection to hub dont run 
                this.hubConnection.stop().catch(error => 
                    console.log('Error stopping connection: ', error))
            }
        }
    }));

    useEffect(() => {//runs when page loads, aaaaaaaaaaaaaanddd runs when the states: activityID or commentstore changes
        if (activityId && !created.current) {//if there is an activity and there is no connection already 
            commentStore.createHubConnection(activityId); // connect to the signalR hub ussing the activityID 
            created.current = true //then sets the created current to true  
        }

        return () => {
            commentStore.stopHubConnection(); //when the component unmounts ie changes page or closes it stops the connection to save resources and
            commentStore.comments = [];// clears the comments arra to save resoruces 
        }
    }, [activityId, commentStore])// and run 

    return {
        commentStore
    }
}