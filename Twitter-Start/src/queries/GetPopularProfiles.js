import { useEffect, useState } from "react";
import { GraphQLClient, gql } from "graphql-request";
import { useMoralis } from "react-moralis";

// CyberConnect Protocol endpoint
const CYBERCONNECT_ENDPOINT = "https://api.cybertino.io/connect/";

// Initialize the GraphQL Client
const client = new GraphQLClient(CYBERCONNECT_ENDPOINT);

// You can add/remove fields in query
export const GET_POPULAR = gql`
    query($fromAddr: String!, $first: Int, $tags: TagsInput!) {
        popular(fromAddr: $fromAddr, first: $first, tags: $tags) {
            list {
                address
                domain
                avatar
                recommendationReason
                isFollowing
            }
        }
    }
`;

export default function GetPopularProfiles() {
    // User account
    const { Moralis} = useMoralis();
    const user = Moralis.User.current();

    const [popularProfiles, setPopularProfiles] = useState([]);

    useEffect(() => {
        if(!user?.attributes?.ethAddress) return;

        client
        .request(GET_POPULAR, {
            fromAddr: user.attributes.ethAddress,
            first: 5,
            tags: {
                list: ["PLAZA"]
            }
        })
        .then((res) => {
            setPopularProfiles(res.popular.list);
        })
        .catch((err) => {
            console.error(err);
        });
    }, [user]);

    return popularProfiles;
}
