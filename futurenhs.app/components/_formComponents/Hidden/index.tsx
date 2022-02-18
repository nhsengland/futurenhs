import { Props } from './interfaces';

export const Hidden: (props: Props) => JSX.Element = ({
    id,
    input: {
        name,
        value
    }
}) => {

    return (
 
        <input 
            id={id} 
            name={name}
            type="hidden" 
            value={value} />
        
    )

}
