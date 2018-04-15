contract ProtoypeEmpData {

    string private _data;
    bytes private _hash;
    address _creator;
    
    function ProtoypeEmpData(string data, bytes hash) public {
        _data = data;
        _hash = hash;
        _creator = msg.sender;
    }
    
    function char(byte b) constant returns (byte c) {
        if (b < 10) return byte(uint8(b) + 0x30);
        else return byte(uint8(b) + 0x57);
    }

    function getCreator() constant public returns (string){
        
        bytes memory s = new bytes(40);
        
        for (uint i = 0; i < 20; i++) {
            byte b = byte(uint8(uint(_creator) / (2**(8*(19 - i)))));
            byte hi = byte(uint8(b) / 16);
            byte lo = byte(uint8(b) - 16 * uint8(hi));
            s[2*i] = char(hi);
            s[2*i+1] = char(lo);            
        }
        
        return string(s);
    }
    
    function getHash() constant public returns (bytes){
        return _hash;
    }
    
    function getData() constant public returns (string){
        return _data;
    }

}
