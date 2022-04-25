import React from "react";
import "./TweetInFeed.css";
import golf from "../images/golf.png";
import canoe from "../images/canoe.png";
import { defaultImgs } from "../defaultimgs";
import { Icon } from "web3uikit";
import { useMoralis } from "react-moralis";
import { useEffect, useState } from "react";

const TweetInFeed = ({ profile }) => {
  const [tweetArr, setTweetArr] = useState();
  const { Moralis, account } = useMoralis();

  useEffect(() => {
    async function getTweets() {
      try {
        const Tweets = Moralis.Object.extend("Tweets");
        const query = new Moralis.Query(Tweets);
        if (profile) {
          query.equalTo("tweeterAcc", account);
        }
        const results = await query.find();

        setTweetArr(results);
        console.log(results);
      } catch (error) {
        console.error(error);
      }
    }
    getTweets();
  }, [profile]);

  return (
    <>
      {tweetArr?.map((e) => {
        return (
          <>
            <div className="feedTweet">
              <img src={e.attributes.tweeterPfp ? e.attributes.tweeterPfp : defaultImgs[0]} className="profilePic"></img>
              <div className="completeTweet">
                <div className="who">
                {e.attributes.tweeterUserName.slice(0, 6)}
                  <div className="accWhen">{
                        `${e.attributes.tweeterAcc.slice(0, 4)}...${e.attributes.tweeterAcc.slice(38)} · 
                        ${e.attributes.createdAt.toLocaleString('en-us', { month: 'short' })}  
                        ${e.attributes.createdAt.toLocaleString('en-us', { day: 'numeric' })}
                        `  
                      }
                      </div>
                </div>
                <div className="tweetContent">
                {e.attributes.tweetTxt}
                {e.attributes.tweetImg && (
                        <img
                          src={e.attributes.tweetImg}
                          className="tweetImg"
                        ></img>
                      )}
                </div>
                <div className="interactions">
                  <div className="interactionNums">
                    <Icon fill="#3f3f3f" size={20} svg="messageCircle" />
                  </div>
                  <div className="interactionNums">
                    <Icon fill="#3f3f3f" size={20} svg="star" />
                    12
                  </div>
                  <div className="interactionNums">
                    <Icon fill="#3f3f3f" size={20} svg="matic" />
                  </div>
                </div>
              </div>
            </div>
          </>
        );
      }).reverse()}

      {/* 
      <div className="feedTweet">
        <img src={defaultImgs[0]} className="profilePic"></img>
        <div className="completeTweet">
          <div className="who">
            Juhizzz
            <div className="accWhen">0x42..314 · 1h</div>
          </div>
          <div className="tweetContent">
            Nice Day Golfing Today Shot 73 (+2)
            <img src={golf} className="tweetImg"></img>
          </div>
          <div className="interactions">
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="messageCircle" />
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="star" />
              12
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="matic" />
            </div>
          </div>

        </div>
      </div>
      <div className="feedTweet">
        <img src={defaultImgs[0]} className="profilePic"></img>
        <div className="completeTweet">
          <div className="who">
            Juhizzz
            <div className="accWhen">0x42..314 · 1h</div>
          </div>
          <div className="tweetContent">
            is simply dummy text of the printing and typesetting industry. Lorem
            Ipsum has been the industry's standard dummy text ever since the
            1500s, when an unknown printer took a galley of type and scrambled
            it to make a type specimen book. It has survived not only five
            centuries, but also the leap into electronic typesetting, remaining
            essentially un
          </div>
          <div className="interactions">
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="messageCircle" />
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="star" />
              12
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="matic" />
            </div>
          </div>
        </div>
      </div>
      
      <div className="feedTweet">
        <img src={defaultImgs[0]} className="profilePic"></img>
        <div className="completeTweet">
          <div className="who">
            Juhizzz
            <div className="accWhen">0x42..314 · 1h</div>
          </div>
          <div className="tweetContent">
            Thoughts on the new Coca-Cola banana 🥤🍌 flavor?
          </div>
          <div className="interactions">
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="messageCircle" />
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="star" />
              12
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="matic" />
            </div>
          </div>
        </div>
      </div>
      <div className="feedTweet">
        <img src={defaultImgs[0]} className="profilePic"></img>
        <div className="completeTweet">
          <div className="who">
            Juhizzz
            <div className="accWhen">0x42..314 · 1h</div>
          </div>
          <div className="tweetContent">
            Love spending time on the water 🌊🌅
            <img src={canoe} className="tweetImg"></img>
          </div>
          <div className="interactions">
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="messageCircle" />
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="star" />
              12
            </div>
            <div className="interactionNums">
              <Icon fill="#3f3f3f" size={20} svg="matic" />
            </div>
          </div>
        </div>
      </div> */}
    </>
  );
};

export default TweetInFeed;
