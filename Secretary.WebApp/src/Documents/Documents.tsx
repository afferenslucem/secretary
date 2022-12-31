import { Route, Routes } from 'react-router-dom';
import TimeOff from './TimeOff/TimeOff';

export default function Documents () {
    return (
        <Routes>
            <Route path="/time-off" element={<TimeOff />}></Route>
        </Routes>
    )
}
