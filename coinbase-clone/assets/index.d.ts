import { ImageSourcePropType } from 'react-native';

declare module '*.png' {
  export default ImageSourcePropType;
}

declare module '*.jpg' {
  export default ImageSourcePropType;
}

declare module '*.jpeg' {
  export default ImageSourcePropType;
}

declare module '*.gif' {
  export default ImageSourcePropType;
}

declare module '*.mp4' {
  export default unknown;
}