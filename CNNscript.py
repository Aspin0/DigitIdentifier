import torch
import torch.nn as nn 
import torch.nn.functional as F

import cv2

import torchvision
import torchvision.transforms as transforms

class CNN(nn.Module):
    def __init__(self):
        super(CNN, self).__init__()

        self.drop = nn.Dropout2d()

        self.convLayer1 = nn.Conv2d(in_channels=1, out_channels=10, kernel_size=5)
        self.convLayer2 = nn.Conv2d(in_channels=10, out_channels=20, kernel_size=5)

        self.fc1 = nn.Linear(in_features=320, out_features=50)
        self.fc2 = nn.Linear(in_features=50, out_features=10)    

    def forward(self, x):
        x = F.relu(F.max_pool2d(self.convLayer1(x), kernel_size=2))
        x = F.relu(F.max_pool2d(self.drop(self.convLayer2(x)), kernel_size=2))
        x = x.view(-1, 320)
        x = F.relu(self.fc1(x))
        x = F.dropout(x, training=self.training)
        x = self.fc2(x)
        return F.log_softmax(x)

def predict():
    model = CNN()
    # change file path here to wherever the model.pt is saved
    model.load_state_dict(torch.load('C:/Users/Lucas/Documents/DigitsCNNproject/model.pt', weights_only=True))
    # make sure this is the same as where the image is saved in the c# file 
    img_path = 'C:/Users/Lucas/Documents/DigitsCNNproject/drawing.png'

    img = cv2.imread(img_path)
    img = cv2.resize(img, (28, 28), interpolation=cv2.INTER_AREA)
    img = cv2.bitwise_not(img)
    img = img[:,:,0] # makes it one channel (grayscale)

    # turn image in tensor
    tf = transforms.Compose([transforms.ToTensor()])
    img_tf = tf(img).float().unsqueeze(0)

    model.eval()
    with torch.no_grad():
        output = model.forward(img_tf)


    prediction = output.data.max(1, keepdim=True)[1][0][0].item()
    print(prediction)
    return(prediction)

if __name__ == '__main__':
    predict()