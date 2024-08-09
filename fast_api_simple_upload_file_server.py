from fastapi import FastAPI, File, UploadFile
from typing import List
import os
import uvicorn

app = FastAPI()

upload_direc = "/home/esec-ai/ros_ws/src/testing_code_transfer/tmp"

os.makedirs(upload_direc, exist_ok=True)

@app.post("/uploadfile")
async def upload_files(files: List[UploadFile] = File(...)):
    file_paths = []
    for file in files:
    # Create the full file path
        file_path = os.path.join(upload_direc, file.filename)
        with open(file_path, 'wb') as f:
            while content := await file.read(1024):
                f.write(content)
            file_paths.append(file_path)
    return {"file_paths": file_paths}

if __name__ == "__main__":
    uvicorn.run(app, host = "0.0.0.0",port=8000)