
�


chat.proto	cmd.proto"�
ChatMessageItem
UniqueID (	RUniqueID3
ChannelType (2.ChatChannelTypesRChannelType
	ChannelID (	R	ChannelID'
Sender (2.PlayerBaseInfoRSender+
Receiver (2.PlayerBaseInfoRReceiver
Content (	RContent"�
ChatMessageList3
ChannelType (2.ChatChannelTypesRChannelType
	ChannelID (	R	ChannelID,
Messages (2.ChatMessageItemRMessages"�
CS_ChatSendMessage3
ChannelType (2.ChatChannelTypesRChannelType
	ChannelID (	R	ChannelID"
ReceiverUUID (	RReceiverUUID
Content (	RContent"<
CS_ChatSwitchPublicChannelID
	ChannelID (	R	ChannelID"9
SC_ChatSync*
Chanels (2.ChatMessageListRChanels"�
ChatPublicChannelInfo
ChanelID (	RChanelID

ChanelName (	R
ChanelNameF
ChanelState (2$.ChatPublicChannelInfo.ChannelStatesRChanelState"9
ChannelStates
NONE 	
GREEN

YELLOW
RED"�
SC_ChatPublicChannelInfo(
All (2.ChatPublicChannelInfoRAll0
History (2.ChatPublicChannelInfoRHistory"
NowChannelID (	RNowChannelID"D
SC_ChatNotifyMessage,
Messages (2.ChatMessageItemRMessages*�
ChatChannelTypes
CHATCHANNELTYPE_UNDEFINE 
CHATCHANNELTYPE_PUBLIC
CHATCHANNELTYPE_PERSONAL
CHATCHANNELTYPE_ROOM
CHATCHANNELTYPE_GUILDBZ
server/cmdbproto3