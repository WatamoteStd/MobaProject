
using System.Buffers.Binary;
using System.Text;
using Moba.Shared.UdpServer.Teams;

namespace Moba.Shared.UdpServer.Packets.Lobby;

public struct S2C_LobbyJoinRequestPacket
{
    
    public byte PlayersCount;
    public ushort[] Ids;
    public MatchTeams[] Teams;
    public string[] Nicknames;

    public int Serialize(Span<byte> buffer)
    {
        
        int offset = 0;

        buffer[0] = PlayersCount;
        offset++;

        for(int i = 0; i < PlayersCount; i++)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer[offset..], Ids[i]);
            offset+=2;
        }

        for(int i = 0; i < PlayersCount; i++)
        {
            buffer[offset] = (byte)Teams[i];
            offset++;
        }

        for(int i = 0; i < PlayersCount; i++)
        {
            string nick = Nicknames[i];
            byte nickLength = (byte)Encoding.UTF8.GetByteCount(nick);
            
            buffer[offset] = nickLength;
            offset++;

            int writtenBytes = Encoding.UTF8.GetBytes(nick, buffer[offset..]);
            offset += writtenBytes;
        }

        return offset;

    }

    public void Deserialize(ReadOnlySpan<byte> buffer)
    {
        
        int offset = 0;

        PlayersCount = buffer[0];
        offset++;

        Ids = new ushort[PlayersCount];
        Teams = new MatchTeams[PlayersCount];
        Nicknames = new string[PlayersCount];

        for(int i = 0; i < PlayersCount; i++)
        {
            Ids[i] = BinaryPrimitives.ReadUInt16LittleEndian(buffer[offset..]);
            offset+=2;
        }
        for(int i = 0; i < PlayersCount; i++)
        {
            Teams[i] = (MatchTeams)buffer[offset];
            offset++;
        }
        for(int i = 0; i < PlayersCount; i++)
        {
            int nicknameLength = buffer[offset];
            offset++;

            Nicknames[i] = Encoding.UTF8.GetString(buffer.Slice(offset, nicknameLength));
            offset += nicknameLength;

        }

    }

}