import { Props } from './interfaces';

export const HiddenInput: (props: Props) => JSX.Element = ({
    input: {
        name,
        value
    }
}) => {

    const id: string = name;

    return (
 
        <input 
            id={id} 
            name={name}
            type="hidden" 
            value={value} />
        
    )

}
