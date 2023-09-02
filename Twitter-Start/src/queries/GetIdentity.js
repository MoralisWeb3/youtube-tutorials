import { useEffect, useState } from "react";
import { GraphQLClient, gql } from "graphql-request";
import { useMoralis } from "react-moralis";

// CyberConnect Protocol endpoint
const CYBERCONNECT_ENDPOINT = "https://api.cybertino.io/connect/";

// Initialize the GraphQL Client
const client = new GraphQLClient(CYBERCONNECT_ENDPOINT);

// You can add/remove fields in query
export const GET_IDENTITY = gql`
    query($address: String!) {
        identity(address: $address) {
            domain
            followerCount
            followingCount
        }
    }
`;

export default function GetIdentity() {
    // User account
    const { Moralis} = useMoralis();
    const user = Moralis.User.current();

    const [identity, setIdentity] = useState({});

    useEffect(() => {
        if(!user?.attributes?.ethAddress) return;

        client
        .request(GET_IDENTITY, {
            address: user.attributes.ethAddress
        })
        .then((res) => {
            setIdentity(res?.identity);
        })
        .catch((err) => {
            console.error(err);
        });
    }, [user]);

    return {
        followingCount: identity.followingCount,
        followerCount: identity.followerCount
    }
}
