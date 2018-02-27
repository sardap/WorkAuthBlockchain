pragma solidity ^0.4.18;

contract ProtoypeEmpData {
    
    string private _data;
    uint256 private _hash;
    
    function getHash() constant public returns (uint256){
        return _hash;
    }
    
    function getData() constant public returns (string){
        return _data;
    }
    
    function ProtoypeEmpData(string data, uint256 hash) public {
        _data = data;
        _hash = hash;
    }
}
